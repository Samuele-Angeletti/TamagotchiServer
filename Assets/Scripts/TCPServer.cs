using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TCPServer : MonoBehaviour, ISubscriber
{
    public string DNSPing = "192.168.1.1";
    public string IP = "192.168.1.217"; // TODO: usare la connessione dinamica all'IP
    public int Port = 5000;

    [SerializeField] Image connectionImage;
    [SerializeField] TextMeshProUGUI connectionMessage;
    [SerializeField] Color connectedColor = Color.green;
    [SerializeField] Color disconnectedColor = Color.red;

    private bool connected = false;
    Coroutine _listenForIcomingRequest;
    private TcpListener tcpListener;
    private TcpClient _connectedTcpClient;

    private void Start()
    {
        Publisher.Subscribe(this, typeof(CharacterUpdateMessage));

        connectionImage.color = disconnectedColor;
        _listenForIcomingRequest = StartCoroutine(ListenForIncomingRequests());
    }

    private void Update()
    {
        if (!connected && _listenForIcomingRequest == null)
        {
            _listenForIcomingRequest = StartCoroutine(ListenForIncomingRequests());
        }
    }

    private IEnumerator ListenForIncomingRequests()
    {
        tcpListener = new TcpListener(IPAddress.Parse(IP), Port);
        tcpListener.Start();

        // sarebbe da inserire controlli di riconnessione
        connected = true;
        connectionImage.color = connectedColor;
        connectionMessage.text = $"Connesso a {IP}:{Port}";

        while (true)
        {
            if (tcpListener.Pending())
            {
                TcpClient incomingClient = tcpListener.AcceptTcpClient();
                StartCoroutine(HandleClient(incomingClient));
            }

            yield return null;
        }


        //_listenForIcomingRequest = null;
    }

    private IEnumerator HandleClient(TcpClient incomingClient)
    {
        NetworkStream stream = incomingClient.GetStream();
        StringBuilder dataBuffer = new StringBuilder();

        while (incomingClient.Connected)
        {
            if (stream.DataAvailable)
            {
                byte[] bytes = new byte[4096];
                int length = stream.Read(bytes, 0, bytes.Length);

                if (length > 0)
                {
                    // accumula i dati ricevuti
                    dataBuffer.Append(Encoding.UTF8.GetString(bytes, 0, length));

                    // verifica se il messaggio ricevuto è completo
                    string data = dataBuffer.ToString();

                    while (true)
                    {
                        int delimiterIndex = data.IndexOf("\n");

                        // se il delimitatore non è ancora arrivato, allora accumula ancora dati
                        if (delimiterIndex == -1)
                            break;

                        // messaggio completo
                        string jsonClientMessage = data.Substring(0, delimiterIndex);

                        dataBuffer.Remove(0, delimiterIndex + 1);
                        data = dataBuffer.ToString();

                        try
                        {
                            CharacterModel characterModel = JsonConvert.DeserializeObject<CharacterModel>(jsonClientMessage);
                            // invio messaggio arrivato a tutti
                            Publisher.Publish(new CharacterModelReceivedMessage(characterModel));

                            _connectedTcpClient = incomingClient;

                            break;
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
            }

            yield return null;
        }

        incomingClient.Close();
    }

    public void OnPublish(IPublisherMessage message)
    {
        if (message is CharacterUpdateMessage characterUpdate)
        {
            SendMessageToClient(characterUpdate.CharacterModel);
        }
    }

    private async void SendMessageToClient(CharacterModel characterModel)
    {
        if (_connectedTcpClient == null || !_connectedTcpClient.Connected)
        {
            Debug.LogError("Client non connesso o perso");
            return;
        }

        string jsonMessage = JsonConvert.SerializeObject(characterModel);

        if (string.IsNullOrEmpty(jsonMessage))
        {
            Debug.LogError("Errore nella conversione del JSON");
            return;
        }

        try
        {
            var stream = _connectedTcpClient.GetStream();
            if (stream.CanWrite)
            {
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonMessage);
                await stream.WriteAsync(jsonByteArray, 0, jsonByteArray.Length);
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public void OnDisableSubscriber()
    {
        Publisher.Unsubscribe(this, typeof(CharacterUpdateMessage));
    }

    private void OnDisable()
    {
        OnDisableSubscriber();
    }
}
