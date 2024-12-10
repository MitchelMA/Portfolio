## Introductie {#iductie}
<p style="text-align: justify;">
Csv, <a href="https://nl.wikipedia.org/wiki/Kommagescheiden_bestand" target="_blank">comma-seperated values</a>,
is een lightweight bestands format om gegevens in op te slaan. Het kan bijvoorbeeld gebruikt worden om waardes te kunnen mappen
(input waarde die gebonden is aan een output waarde). Ik maak er ook gebruik van in dit project om
kleurecodes toe te passen op de indicator die je linksonderin ziet.  
</p>
  
![Voorbeeld van een .csv bestand](./images/rider64_6CXTNg5dUr.png)

## Standaard {#stand}
Er is op het moment wel een idee voor een "officiële"
<a href="https://datatracker.ietf.org/doc/html/rfc4180">definitie (RFC 4180)</a>
voor .csv bestanden, maar over het algemeen wordt hier vaak van afgeweken.  

<hr />

De standaarden zijn als volgt:  
  
 - Csv is gewone tekst, net zoals .txt bestandjes op computers
 - Het bestaat uit aantekeningen (1 per regel is gebruikelijk)
 - De aantekening van een regel zijn opgesplits in stukjes middels een scheidingsteken (dit is gebruikelijk één teken als een komma)
 - Een \`<a href="https://developer.mozilla.org/en-US/docs/Glossary/String" target="_blank">string</a>\` wordt geöpend en gesloten met een aanhalingsteken, en mag voorbij het einde van een regel lopen
 - In het geval dat je een aanhalingsteken wilt opnemen in een string, gebruik je er twee achter elkaar: "John ""Doe""" = John "Doe". Dit heet een <a href="https://en.wikipedia.org/wiki/Escape_sequence" target="_blank">escape sequence</a>
  
Op het moment wijk ik ook af van deze standaard. Ik heb iets toegevoegd als \`Comments\`, kleine stukjes tekst
die bedoelt zijn om de code te verduidelijken.

## Voorbeeld {#ex}
Ik heb ook nog een klein voorbeeld om aan te geven hoe dit er precies uitziet.
Het eerste plaatje is het csv-bestandje en het tweede plaatje een voorbeeld van hoe ik het kan
uitlezen in code.
Dit wordt ook wel een model genoemd.  
  
![Voorbeeld van een .csv bestand](./images/rider64_6CXTNg5dUr.png)  
  
![Voorbeeld van een model in code](./images/rider64_DgcDDkJOS9.png)