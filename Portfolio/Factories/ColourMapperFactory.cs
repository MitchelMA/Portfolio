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

   public ColourMapper<T> CreateMap<T>(string fileHref) 
      where T : Enum
   {
      throw new NotImplementedException();
   }

   private Dictionary<T, Color> Lexer<T>(string fileText)
      where T : Enum
   {
      throw new NotImplementedException();
   }
}