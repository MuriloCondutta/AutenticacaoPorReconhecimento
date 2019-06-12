using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AutenticacaoPorReconhecimento.Utils
{
    public class ImagemUtils
    {
        const string subscriptionKey = "9b4423e2c61f4b779b8480394081e7e8";
        const string uriBase = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/";
        public string JSONresponse;

        public Image Base64ParaImagem(string imagemBase64)
        {

            // Converte para byte[]
            byte[] imageBytes = Convert.FromBase64String(imagemBase64);

            // Converte o byte[] para imagem
            // Cria a imagem na memória
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                Image imagem = Image.FromStream(ms, true);
                return imagem;
            }

        }

        public byte[] Base64ParaByte(string imagemBase64)
        {

            string base64 = imagemBase64.Split(',')[1];

            // Converte para byte[]
            byte[] imageBytes = Convert.FromBase64String(base64);

            return imageBytes;

        }

        public async Task MakeAnalysisRequest(byte[] imagem)
        {

            HttpClient cliente = new HttpClient();

            // Request headers
            cliente.DefaultRequestHeaders.Add(
                "Ocp-Apim-Subscription-Key", subscriptionKey);

            // Request Parameters
            string requestParameters = "returnFaceId=true&returnFaceLandmarks=false" +
                "&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses," +
                "emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";

            // Monta a URL
            string uri = uriBase + "detect?" + requestParameters;

            HttpResponseMessage response;

            // Posta uma imagem JPEG em byte[]
            // parâmetro imagem já está em byte[]
            using (ByteArrayContent content = new ByteArrayContent(imagem))
            {
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                // Chama a API
                response = await cliente.PostAsync(uri, content);

                // Recebe a resposta
                string contentString = await response.Content.ReadAsStringAsync();

                JSONresponse = contentString;

            }

        }

        public string PegarFaceId(string corpoResposta)
        {

            string faceId = corpoResposta.Split('\"')[3];

            return faceId;
        }

        public string PegarPesonId(string corpoResposta)
        {

            string personId = corpoResposta.Split('\"')[3];

            return personId;
        }

        public async Task CriarGrupoDePessoas(string nomeGrupo, string nomeFantasiaGrupo)
        {
            HttpClient cliente = new HttpClient();

            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            cliente.DefaultRequestHeaders.Add(
                "Ocp-Apim-Subscription-Key", subscriptionKey);

            // Monta a URL
            string uri = uriBase + "persongroups/" + nomeGrupo + "?" + queryString;

            HttpResponseMessage response;

            // Request body
            string body = "{" +
                            "\"name\": \"" + nomeFantasiaGrupo + "\"" +
                            "}";

            byte[] byteData = Encoding.UTF8.GetBytes(body);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                    "application/json");

                response = await cliente.PutAsync(uri, content);

                // Recebe a resposta
                string contentString = await response.Content.ReadAsStringAsync();

                JSONresponse = contentString;
            }
        }

        public async Task CriarPessoa(string nomePessoa, string nomeGrupo)
        {
            HttpClient cliente = new HttpClient();

            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            cliente.DefaultRequestHeaders.Add(
                "Ocp-Apim-Subscription-Key", subscriptionKey);

            // Monta a URL
            string uri = uriBase + "persongroups/" + nomeGrupo + "/persons?" + queryString;

            HttpResponseMessage response;

            // Request body
            string body = "{\"name\" : \"" + nomePessoa + "\"}";

            byte[] byteData = Encoding.UTF8.GetBytes(body);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                    "application/json");

                response = await cliente.PostAsync(uri, content);

                // Recebe a resposta
                string contentString = await response.Content.ReadAsStringAsync();

                JSONresponse = contentString;
            }
        }

        /**
         * Cadastra uma face a uma pessoa já existente
         */ 
        public async Task CadastrarFace(string pessoaId, string grupoId, byte[] imagem)
        {
            HttpClient cliente = new HttpClient();

            // Request headers
            cliente.DefaultRequestHeaders.Add(
                "Ocp-Apim-Subscription-Key", subscriptionKey);

            // Monta a URL
            string uri = "https://westus.api.cognitive.microsoft.com/face/v1.0/persongroups/" + grupoId + "/persons/" + pessoaId + "/persistedFaces?";

            HttpResponseMessage response;

            // Posta uma imagem JPEG em byte[]
            // parâmetro imagem já está em byte[]
            using (ByteArrayContent content = new ByteArrayContent(imagem))
            {
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                // Chama a API
                response = await cliente.PostAsync(uri, content);

                // Recebe a resposta
                string contentString = await response.Content.ReadAsStringAsync();

                JSONresponse = contentString;

            }

        }

        public async Task TreinarGrupoDePessoas(string grupoId)
        {
            // Instancia o cliente
            HttpClient cliente = new HttpClient();

            // Monta o Header
            cliente.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            // Monta a URL
            string uri = uriBase + "persongroups/" + grupoId + "/train";

            // Instancia a resposta
            HttpResponseMessage response;

            // Realiza a requisição e recebe a resposta
            byte[] byteData = Encoding.UTF8.GetBytes("");

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                // Chama a API
                response = await cliente.PostAsync(uri, content);

                // Recebe a resposta
                string contentString = await response.Content.ReadAsStringAsync();

                JSONresponse = contentString;

            }

        }

        public async Task StatusTreinoGrupoDePessoas(string grupoId)
        {
            // Instancia o cliente
            HttpClient cliente = new HttpClient();

            // Monta o Header
            cliente.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            // Monta a URL
            string uri = uriBase + "persongroups/" + grupoId + "/training";

            // Instancia a resposta
            HttpResponseMessage response;

            // Pede a resposta
            response = await cliente.GetAsync(uri);

            // Recebe a resposta
            string contentString = await response.Content.ReadAsStringAsync();

            JSONresponse = contentString;

        }

        // É possível receber o status do treinamento

        public async Task IdentificarPessoa(string nomeGrupo, string faceId, byte[] imagem)
        {

            HttpClient cliente = new HttpClient();

            cliente.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            string uri = uriBase + "identify";

            HttpResponseMessage response;

            string body = "{ " +
                            " \"personGroupId\": \"" + nomeGrupo  + "\", " +
                            " \"faceIds\": [\"" +
                                faceId +
                            "\"], " +
                            " \"maxNumOfCandidatesReturned\": 1, \"confidenceThreshold\": 0.5" +
                            "}";

            byte[] corpoByte = Encoding.UTF8.GetBytes(body);

            using (ByteArrayContent content = new ByteArrayContent(corpoByte))
            {
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                // Chama a API
                response = await cliente.PostAsync(uri, content);

                // Recebe a resposta
                string contentString = await response.Content.ReadAsStringAsync();

                JSONresponse = contentString;

            }

        }

        public async Task pegarGrupoDePessoas(string nomeGrupo)
        {

            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            // Request parameters
            queryString["returnRecognitionModel"] = "false";
            var uri = uriBase + "persongroups/" + nomeGrupo + "?" + queryString;

            var response = await client.GetAsync(uri);

            JSONresponse = await response.Content.ReadAsStringAsync();

        }

    }
}