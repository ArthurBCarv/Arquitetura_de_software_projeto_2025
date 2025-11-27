using System.Text.Json;

namespace Compras.Services
{
    public class UsuariosService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public UsuariosService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<UsuarioResponse?> GetUsuarioByIdAsync(int usuarioId)
        {
            try
            {
                var usuariosUrl = _configuration["Services:UsuariosUrl"] ?? "http://localhost:5000";
                var response = await _httpClient.GetAsync($"{usuariosUrl}/api/usuarios/{usuarioId}");
                
                if (!response.IsSuccessStatusCode)
                    return null;

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<UsuarioResponse>(content, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> AdicionarJogoBibliotecaAsync(int usuarioId, int jogoId)
        {
            try
            {
                var usuariosUrl = _configuration["Services:UsuariosUrl"] ?? "http://localhost:5000";
                var payload = new { JogoId = jogoId };
                var content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync(
                    $"{usuariosUrl}/api/usuarios/{usuarioId}/biblioteca",
                    content
                );

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }

    public class UsuarioResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
        public string Email { get; set; } = "";
        public int Pontos { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
