using AutenticacaoPorReconhecimento.Models;
using AutenticacaoPorReconhecimento.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AutenticacaoPorReconhecimento.Controllers
{
    public class UsuarioController : Controller
    {

        List<Usuario> usuarios = new List<Usuario>() {

            new Usuario {Nome = "Murilo", Email = "muri_ac@hotmail.com", Senha = "senha123", FaceGroupId = "usuario_administrador"}

        };

        // utilizando FACE API
        ImagemUtils imagemUtils = new ImagemUtils();

        // GET: Carregar Face API
        public async Task<ActionResult> Iniciar()
        {
            imagemUtils.JSONresponse = "olá";

            await imagemUtils.CriarGrupoDePessoas("usuario_administrador", "administradores");

            await imagemUtils.CriarPessoa("Murilo", "usuario_administrador");

            //string respostaCriarPessoa = imagemUtils.JSONresponse;

            //usuarios[0].PersonIdFace = imagemUtils.PegarPesonId(respostaCriarPessoa);

            await imagemUtils.TreinarGrupoDePessoas("usuario_administrador");

            await Task.Delay(1000);

            await imagemUtils.StatusTreinoGrupoDePessoas("usuario_administrador");

            ViewData["JSONresposta"] = imagemUtils.JSONresponse;
            return View("Login");
        }

        // GET: Usuario
        public ActionResult Login()
        {
            return View(usuarios[0]);
        }
        
        [HttpPost]
        public async Task<ActionResult> Entrar(Usuario usuario)
        {

            try
            {
                // Transforma a base64 em byte[]
                byte[] imagem = imagemUtils.Base64ParaByte(usuario.ImagemBase64);

                // Solicita a API de detecção
                await imagemUtils.MakeAnalysisRequest(imagem);

                // Pega o face id
                string faceId = imagemUtils.PegarFaceId(imagemUtils.JSONresponse);

                // Envia para detecção
                await imagemUtils.IdentificarPessoa("usuario_administrador", faceId, imagem);

            }
            catch (Exception)
            {
                throw;
            }

            // Recebe a resposta do ImagemUtils
            ViewData["JSONresposta"] = imagemUtils.JSONresponse;
            ViewData["FaceId"] = imagemUtils.PegarFaceId(imagemUtils.JSONresponse);

            // Retorna a View
            return View(usuario);

        }

        public ActionResult Cadastro()
        {
            return View(usuarios[0]);
        }

        public async Task<ActionResult> Cadastrar(Usuario usuario)
        {

            await imagemUtils.CadastrarFace(usuarios[0].PersonIdFace, "usuario_administrador", imagemUtils.Base64ParaByte(usuario.ImagemBase64));

            ViewData["JSONresposta"] = imagemUtils.JSONresponse;
            return View("Login");
        }

    }

}