using Portfolio.Model.Tags;

namespace Portfolio.Client;

public static class StaticData
{
   public const int DefaultLangCode = 0;
   public const string PageTitle = "Portfolio";
   public static readonly LinkTag DefaultPageIcon = new PageIcon("image/webp", "./images/AboutMeCroppedHead.webp");
   public const string MainHeaderImgPath = "./images/cmd_CaIolk4veY.png";
   public const string ScrollDownImage = "./images/scrolldown_image.png";
   public const string LockedClassName = "scroll-locked";
   public const string GitHubStartLink = "";
}