## Introductie {#iductie}
<p style="text-align: justify">
In de programmeertaal C bestaat er geen concept van wat een string nou echt is:
het enige wat de taal kent, is een <code>char</code>-array die eindigt op een zogehete <code>null</code>-terminator.
Om dus te zorgen dat ik efficient, makkelijk, maar bovenal veilig kan werken met stukken tekst in C; heb ik deze library
voor mezelf geschreven.
Het hoort bij de verzameling van door mij geschreven libraries waar ik vaak gebruik van maak:
</p>
 - [C-Vector](./project/cvector)
 - [Commandline Arg Parser](./project/cap)

## Structures {#structs}
Voor een bepaalde vorm van harmonie in het gebruik van de library, zijn er een aantal structures
nodig die met elkaar samen werken:  

 - een "owning" string: `string_t`
 - een "non-owning/slice" string: `stringview_t`
 - een "growing" string: `stringbuilder_t`

![Types in de library](./images/cstring/types.png "open")  
Deze structures bieden de mogelijkheid aan om met een "owning" string te werken,
een "non-owning" of "slice" door te geven aan een functie als aangemaakt vanaf een owning string;
of zelfs veilig werken met gebruikers input van `stdin` door de `stringbuilder` te
gebruiken.

## Initialization & Destruction {#initdes}
Voor strings wordt gebruik gemaakt van de heap, omdat het niet betrouwbaar is om een grote stack-allocation te
maken&mdash;naast het feit dat een stack-allocation niet langer dan de stack leeft...  
Een `string_t` is de eigenaar van zijn buffer. Deze buffer leeft zoals net aangeven op de heap,
en moet dus op weer opgeschoont worden door de gebruiker.
Een `stringview_t` heeft daarintegen geen clean function nodig; omdat een stringview niet de eigenaar is
van de string zelf noch de buffer.
Voor de `stringbuilder_t` wordt gebruik gemaakt van een [dynamic array](./project/cvector) die ik zelf geschreven heb.
Ook deze moet weer opgeruimt worden wanneer je klaar bent.

![string_t initializatie en clean](./images/cstring/string_init_clean.png "open")
![stringview_t initializatie en reset](./images/cstring/stringview_init_reset.png "open")
![stringbuilder_t initializatie en clean](./images/cstring/stringbuilder_init_clean.png "open")

## Nawoord {#naw}
Ik gebruik deze library met regelmaat als ik veel met stukken tekst werk in C, omdat ik er een hoop
makkelijke helper functies in heb gescrheven wat tekstverwerking efficient en gemakkelijk maakt.
Een voorbeeld hiervan zou bijvoorbeeld het kunnen splitten op een delimeter zijn.  
Voor meer detail, bekijk de GitHub pagina waarvan de link bovenaan de pagina staat.