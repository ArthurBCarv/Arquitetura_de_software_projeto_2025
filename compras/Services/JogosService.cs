using System.Text.Json;

namespace Compras.Services
{
    public class JogosService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public JogosService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<JogoResponse?> GetJogoByIdAsync(int jogoId)
        {
            try
            {
                var jogosUrl = _configuration["Services:JogosUrl"] ?? "http://localhost:5001";
                var response = await _httpClient.GetAsync($"{jogosUrl}/api/jogos/{jogoId}");
                
                if (!response.IsSuccessStatusCode)
                    return null;

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<JogoResponse>(content, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
            }
            catch
            {
                return null;
            }
        }
    }

    public class JogoResponse
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = "";
        public string Descricao { get; set; } = "";
        public decimal Preco { get; set; }
        public string Desenvolvedor { get; set; } = "";
        public DateTime DataLancamento { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
