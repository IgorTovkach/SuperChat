using System.Net;
using NAudio.Wave;
using SuperChat.Models.Chats.Base;

namespace SuperChat.Models.Chats.Voice
{
    public sealed class VoiceChat : BaseChat
    {
        private readonly BufferedWaveProvider bufferedWaveProvider =
            new BufferedWaveProvider(new WaveFormat(5000, 32, 1));

        private readonly WaveIn inputWave = new WaveIn();
        private readonly WaveOut outputWave = new WaveOut();

        public VoiceChat()
        {
            InitializeSoundRecord();
            InitializeSoundPlay();
            foreach (var hostAddress in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                EndPoints.Add(new EndPointModel
                {
                    EndPoint = new IPEndPoint(hostAddress, ChatConfigurationManager.Port),
                    IsIgnored = true
                });
            }
        }

        public override string ChatConfigurationFile => "VoiceChat.xml";

        private void InitializeSoundRecord()
        {
            inputWave.BufferMilliseconds = 10;
            inputWave.WaveFormat = new WaveFormat(5000, 32, 1);
            inputWave.StartRecording();
            inputWave.DataAvailable += SendVoice;
        }

        private void InitializeSoundPlay()
        {
            outputWave.Init(bufferedWaveProvider);
            outputWave.Play();
            NewMessage += PlayVoice;
        }

        private void PlayVoice(IPEndPoint endPoint, byte[] message)
        {
            bufferedWaveProvider.AddSamples(message, 0, message.Length);
        }

        private void SendVoice(object sender, WaveInEventArgs e)
        {
            Send(e.Buffer);
        }

        public override void Dispose()
        {
            base.Dispose();
            inputWave.StopRecording();
            Active = false;
            if (outputWave != null)
            {
                outputWave.Stop();
                outputWave.Dispose();
            }
            inputWave?.Dispose();
        }
    }
}