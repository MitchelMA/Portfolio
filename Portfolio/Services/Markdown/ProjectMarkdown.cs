using MarkdownDeep;

namespace Portfolio.Services.Markdown;

public class ProjectMarkdown : MarkdownDeep.Markdown
{
    public override void OnPrepareImage(HtmlTag tag, bool titledImage)
    {
        base.OnPrepareImage(tag, titledImage);
        
        // always set the 'alt' attribute equal to the title if it exists
        if (tag.attributes.TryGetValue("title", out var title))
            tag.attributes.Add("alt", title);
    }
}