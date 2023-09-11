using Portfolio.Model.Tags;

namespace Portfolio.Client;

public static class StaticData
{
   public const int DefaultLangCode = 1;
   public const string PageTitle = "Portfolio";
   public static readonly LinkTag DefaultPageIcon = new PageIcon("image/png", "favicon.png");
   public const string MainHeaderImgPath = "./images/cmd_CaIolk4veY.png";
   public const string ScrollDownImage = "./images/scrolldown_image.png";
   public const string LockedClassName = "scroll-locked";
}