## Introductie {#iductie}
<p style="text-align: justify">
In de programmeertaal C++, heb je in de standard library iets als een
<a href="https://en.cppreference.com/w/cpp/container/vector" target="_blank">std::vector</a>.
Deze vector biedt een makkelijke manier van dynamisch geheugen, net als C#' 
<a href="https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1?view=net-7.0"
   target="_blank">List&ltT&gt</a>.
Hierdoor werd ik dus geïnspireerd om een zelfde soort iets te maken in C (al dan wel iets simpeler).
</p>

## Gebruik {#geb}
Deze library maakt gebruik van CMake, wat ik een fijn build-system vind.
Maar CMake kent nog iets handigs;
<a href="https://cmake.org/cmake/help/latest/command/find_package.html"
   target="_blank"><code>find_package()</code></a>.
Wanneer je de package wilt toevoegen, gebruik je
<a href="https://cmake.org/cmake/help/latest/module/FetchContent.html"
   target="_blank"><code>FetchContent</code></a>.
Daarna kan je de lib `CVector` toevoegen aan je target.

## Structuur {#struct}
De structuur van de vector biedt de mogelijkheid om het te gebruiken voor alle types (niet door elkaar in de zelfde vector).
Dit is omdat het begin address wordt bijgehouden middels een void-pointer. En de grootte van het type wordt gebruikt
voor de pointer-arithmetic om offsets te bepalen. Verder houdt de vector een capacity en een grootte bij.  
De capacity maakt optimization mogelijk. Ik hoef hierdoor niet elke keer wanneer een element wordt toegevoegd, meer geheugen te allocaten.
Pas wanneer hij in de buurt van de capacity komt, verdubbeld hij de capacity. Hetzelfde gaat op voor krimpen.  
  
![Huidig structuur van de vector in de .c file](./images/cvector/header.png)
![De public definition van vector_t om te gebruiken](./images/cvector/public-typedef.png)

## Performance {#perf}
 Door de opzet van de structuur, is de performance van het toevoegen van een element aan het einde van de vector
 O(1), random access is O(1) en het toevoegen van een element in de vector O(n) waar \`n\` het aantal opvolgende elementen
 in de vector. Ook is het mogelijk om een vector in een andere vector te inserten, wat een O(1) performance heeft.
 Het is dus sneller om in een loop te appenden aan een tijdelijke vector, en deze dan te inserten in je persistent vector.

## Destruction {#des}
Eén van de dingen waar je mee moet opletten bij het gebruik van deze struct, is dat het groeiende geheugen op de
<a href="https://www.guru99.com/stack-vs-heap.html" target="_blank">heap</a>
leeft.  
In C is het in deze situatie misschien dus wel onhandig dat een \`struct\` niet zoiets kent als een \`destructor\` member.
Wat betekend dat je dit dus handmatig moet doen. Je moet dus als gebruiker altijd de 
`void vector_clean()` function aanroepen om de vector op te ruimen wanneer je klaar bent met het gebruiken.  
  
![Een van de mogelijke functies om een vector_t te constructen](./images/cvector/constructor.png)  
  
![De functie om een vector_t te destructen](./images/cvector/destructor.png)