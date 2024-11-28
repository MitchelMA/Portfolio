using System.Net.Http.Json;
using Portfolio.Model.Hero;

namespace Portfolio.Services;

public class HeroInfoGetter
{
   private readonly HttpClient _httpClient;
   private HeroManifest _heroManifest;
   private bool _isLoaded = false;
   
   public HeroManifest HeroManifest => _heroManifest;
   public bool IsLoaded => _isLoaded;

   public HeroInfoGetter(HttpClient httpClient)
   {
      _httpClient = httpClient;
   }

   public async Task LoadManifest()
   {
      if (_isLoaded)
         return;
      
      _heroManifest = await _httpClient.GetFromJsonAsync<HeroManifest>($"./HeroData/RegisteredHeroes.json");
      OnManifestLoaded();
   }

   private void OnManifestLoaded()
   {
      _isLoaded = true;
      ManifestLoaded?.Invoke(this);
   }
   
   public delegate void ManifestLoadedDelegate(object sender);
   public event ManifestLoadedDelegate? ManifestLoaded;
}