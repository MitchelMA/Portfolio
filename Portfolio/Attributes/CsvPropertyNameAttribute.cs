namespace Portfolio.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class CsvPropertyNameAttribute : Attribute
{
   internal string? Name;
   
   public CsvPropertyNameAttribute()
   {}

   public CsvPropertyNameAttribute(string name)
   {
      Name = name;
   }
}