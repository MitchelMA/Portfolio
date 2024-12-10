## Introductie {#iductie}
<p style="text-align: justify">
<a href="https://nl.wikipedia.org/wiki/Mijnenveger_(spel)" target="_blank">Minesweeper</a>
is een een spel bestaande uit gesloten vakjes, waar onder enkele willekeurige vakjes een mijn zit.
Het doel van het spel is om alle mijnen te identificeren. Wanneer men een vakje opent waartegen geen mijn grenst,
zal er groep vakjes automatisch geopend worden waaronder ook geen mijn zit. In het geval er een vakje wordt geopend
waar een mijn aan grenst, zal er een nummer tussen 1 en 8 verschijnen. Dit nummer geeft aan hoeveel mijnen er aan dit
vakje grenzen.
Wanneer er een vakje wordt geopend waar een mijn onder zit, is de speler af en het spel voorbij.
</p>

## Input {#i}
Input van de keyboard was een van de dingen die ik al snel moeilijk vond worden.
Voornamelijk ook omdat de manier van het uitlezen van input voor een Linux-systeem zo compleet
anders is dan dat voor een Windows-systeem.  

<hr />

Voor een Windows-systeem is er een ingebouwde API uit de
<a href="https://en.wikipedia.org/wiki/Conio.h" target="_blank">
<span style="font-style: italic">conio.h</span>
</a> header. Deze API,
<a href="https://learn.microsoft.com/en-us/cpp/c-runtime-library/reference/getch-getwch?view=msvc-170"
   target="_blank">
<span style="font-style: italic">_getch</span>
</a>, bied een mogelijkheid om directe keyboard input uit te lezen
zonder dat dit te zien is in de console zelf.
In het geval we het hexidecimale getal 0xE0 uitlezen, verwachten we nog
een "input", dit is het geval voor meeste non-alphanumeric keys van de keyboard.
Als bijvoorbeeld de arrow-keys.  

<hr />

Op een Linux-systeem werkt dit net wat anders.
Ten eerste moet input mode op Non-Canonical staan en moet de ECHO
naar de terminal uit staan. En om de input uit te lezen, lees
ik 8 bytes uit de file-descriptor van stdin (dit is zodat ik non-blocking kan uitlezen). Ook return ik hoe hoog de
"degree" is die ik heb uitgelezen, welke de hoeveelheid aan uitgelezen bytes aangeeft.  
  
![Voorbeeld van struct voor input-gegevens](./images/minesweeper/clion64_ubcBv7IdSa.png)
![Function voor multi-platform input](./images/minesweeper/clion64_7Q11kdq78G.png)

## Output {#o}
Als je naar de background van de header van deze pagina kijkt, zie je dat
er met kleuren naar de console/terminal wordt geprint. Voor zover ik ermee heb gewerkt,
is het enabelen hiervan alleen nodig op een Windows-systeem. De flag die hiervoor aangezet moet worden
heet
<a href="https://learn.microsoft.com/en-us/windows/console/setconsolemode#:~:text=any%20subsequent%20characters.-,ENABLE_VIRTUAL_TERMINAL_PROCESSING,-0x0004"
target="_blank">
ENABLE_VIRTUAL_TERMINAL_PROCESSING
</a>  
  
![Functie om ansi-escape characters te kunnen gebruiken voor output](./images/minesweeper/clion64_AldTn81zFX.png)  
  
![Functie om de console-modus te restoren](./images/minesweeper/clion64_BOeIkOFoon.png)

## Opslaan {#op}
Ik heb het ook mogelijk gemaakt om je spel-voortgang op te kunnen slaan.
Dit Het format ziet er als volgt uit: <br/>
&lt;field-size&gt; &lt;bomb-percentage&gt; &lt;is-field-data-saved?&gt; &lt;seed&gt; &lt;cell-bytes&gt;. <br/>  

<hr/>

Het converten van een Cell naar een Byte is makkelijk, omdat de Cell struct een
<a href="https://en.cppreference.com/w/cpp/language/bit_field" target="_blank">bit-field</a>
is. En omdat alle Cell's naar een byte worden geconvert wanneer ze worden opgeslagen,
zijn opslag-bestanden niet groot. Dus al wil ik een speelveld van 50x50 opslaan.
dan bedraagt dit minder drie kilobyte. Het uitlezen werkt op precies de omgekeerde manier.
zo wordt 0x09 bijvoorbeeld uitgelezen als een geopend vakje welke aan één mijn grenst.  
  
![Indeling van cell-gegevens](./images/minesweeper/clion64_v4vVOEyDjJ.png)
![Explicit constructor om een Cell te creëren van een byte](./images/minesweeper/clion64_lb813o5JLQ.png)
![Function om een Cell naar een byte te converten](./images/minesweeper/clion64_aR5W31UiOI.png)
 