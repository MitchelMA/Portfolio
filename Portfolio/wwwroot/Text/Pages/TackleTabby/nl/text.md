## Examen {#exam}
<p style="text-align: justify;">
Voor ons examen, kregen wij een opdracht vanuit een bedrijf. In ons geval was dit
<a href="https://www.azerion.com/" target="_blank">Azerion</a>.
Zij gaven ons de opdracht om een mobiele spel te maken met een <code>Match-3</code> mechanic als een input
voor het besturen van het spel. In ons geval draaide het om vissen vangen.
</p>

## Game Idee {#idea}
Zoals ik hiervoor al schreef, moest ons spel om een andere hoofd-mechanic draaien;
met Match-3 als de input mechanic. We hadden aan het begin een paar concepten; waar we uiteindelijk een combinatie van hebben gemaakt:

 - Turn-based combat
 - Iets met katten
 - Een dress-up game
 - Een (katten) cafe/bakkerij
 - Vissen

Uiteindelijk hebben we gekozen voor een spel waar je middels de Match-3 mechanic moet kiezen welke `Bait` er gebruikt wordt,
en dan vist `Michael` - de naam van de kat - aan de hand van de gekozen bait een vis op; of afval.  
Het spel draait dus om Michael en zijn reacties op de input van de speler.  
  
Wanneer Michael dus een stukje afval opvist, zal hij de opvis animatie afspelen waar hij aan het einde teleurgesteld kijkt.
Vist Michael w&#233;l een vis op, dan zal deze vis te zien zijn in de opvis animatie; en is Michael vrolijk aan het einde van de animatie.  
De opgeviste vissen kunnen vervolgens teruggevonden worden in de `Catalogue`. Hierin kan een speler zien hoe groot de gevangen vis is, en welk bait er voor gebruikt was.  
  
Uiteindelijk wint de speler door alle 10 de verschillende vissen minstens &#233;&#233;n keer opgevist te hebben.

## Gameplay {#gameplay}
<video controls style="max-height: 700px;"
    poster="./images/TackleTabby/EnvironmentPortrait.png"
    thumbnail="./images/TackleTabby/EnvironmentPortrait.png">
        <source src="./videos/TackleTabby/Sprint3GamePlay.mp4" type="video/mp4"/>
</video>

## Gemaakte Mechanics {#mechanics}
Als volgt een paar mechanics waar ik aan heb gewerkt in dit project:  
#### Match & Validate {#matchnvalidate}
Om een Match-3 spel te maken, moet je logischerwijs wel de matches kunnen detecteren.
Hiervoor kwam ik met twee verschillende idee&#235;n: complex of simpel.
Het complexe idee bestond uit het gebruik van een `node-tree` welke dan de verbindingsrelaties liet zien.
Om te zorgen dat een herkeningssysteem goed tree's kan herkennen, moeten de tree's ook naar een algemen vorm gebracht worden:  
![Transposing node tree](./images/TackleTabby/TransposingNodeTree.png "open")  
Uiteindelijk merkten we al snel dat dit te ingewikkeld werd en we te weinig tijd hiervoor hadden.
  
Het twee idee - de simpele - kwam simpel weg neer op het horizontaal en verticaal zoeken naar soortgelijke types.
De gevonden blokken werden dan bijgehouden in een lijstje, en dit lijstje werd gestuurd naar een ander systeem wat bepaalt of het geldig is of niet:  
![Visual Sheet van Match & Validate](./images/TackleTabby/VSMatchNValidate.png "open")  
<a href="https://github.com/WizelfMike/TackleTabby/wiki/Technical-Design#match-and-validate" target="_blank">Technical-Design</a>

#### Removing, Spawning & Falling
Wanneer de gemaakte match goedgekeurd is, moet deze ook weer opgeschoont worden. Dit wordt gedaan door een ander systeem.
Dit systeem luistert naar de `Validator` om te weten wanneer er een match goedgekeurd is:  
![De match validator in Unity](./images/TackleTabby/ValidatorInEngine.png "open")  
De `Remover` verwijderd de aangegeven blokken uit het speelveld door ze te verplaatsten naar een `Object-Pool`:  
![De Remover met een referencie naar de Object Pool](./images/TackleTabby/RemoverEngine.png "open")  
![Gif van het verwijderen in werking](./images/TackleTabby/Removal.gif "open")  
  
Na dat de blokken verwijderd zijn, moeten ze ook weel teruggeplaatst worden. Dit wordt gedaan door de `Spawner`.
Deze spawner luistert om te weten wanneer de blokken verwijderd zijn, haalt ze weer op uit de voorgenoemde object-pool en spawnt ze weer terug in het spel:  
![Het terug-spawn proces](./images/TackleTabby/SpawningProcess.gif "open")  
Na het spawnen wordt de zwaartekracht toegepast op de blokken, en vallen ze weer het speelveld binnen:  
![De volledige loop van het vallen](./images/TackleTabby/GravityProcess.gif "open")  
De Visual-Sheet van het hele process:  
![Visual Sheet van Removing, Spawning & Falling](./images/TackleTabby/VSReSpFa.png "open")  
<a href="https://github.com/WizelfMike/TackleTabby/wiki/Technical-Design#removing--spawning--falling" target="_blank">Technical-Design</a>

## Art-Referencies {#references}
\<Art-stations\> van de verschillende artists.
