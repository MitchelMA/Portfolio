using System.Drawing;
using Portfolio.Services;

namespace Portfolio.Factories;

public class ColourMapperFactory
{
   private HttpClient _httpClient;
   private Dictionary<Type, ColourMapper<Enum>> _maps = new();
   
   public ColourMapperFactory(HttpClient httpClient)
   {
      _httpClient = httpClient;
   }

   public async Task<ColourMapper<T>> CreateMap<T>(string fileHref) 
      where T : Enum
   {
      if (_maps.ContainsKey(typeof(T)))
         return _maps[typeof(T)];
      
      var fileText = await _httpClient.GetStringAsync(fileHref);
      var outcome = Lexer<T>(fileText);
      
      return new ColourMapper<T>(outcome);
   }

   private Dictionary<T, Color> Lexer<T>(string fileText)
      where T : Enum
   {
      throw new NotImplementedException();
   }
}