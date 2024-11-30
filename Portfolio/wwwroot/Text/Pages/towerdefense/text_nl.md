## Introductie {#iduct}
<span style="text-align: justify;">
    In de eerste periode van het tweede jaar op deze studie, hadden wij
    10 weken de tijd gekregen om aan een 
    <a href="https://en.wikipedia.org/wiki/Tower_defense" target="_blank">tower defense</a>
    spel te werken.
    Het doel van deze opdracht was niet om een mooi uitziend spel te maken,
    maar eerder het leren van implementeren van 
    <a href="https://en.wikipedia.org/wiki/Game_mechanics" target="_blank">game mechanics</a>
    (i.e. enemy-wave systeem; schieten van projectielen; etc..)  
</span>
![De map van het spel](./images/Unity_4xajzJxaDR.jpg)

## Path Finding {#path}
Om enemies een pad af te kunnen laten lopen, is er een systeem nodig
om aan te geven waar het pad is. <br/>
Volgorde is alvolgt: een enemy wordt ingespawned; deze krijgt zijn eerste "target"
doorgegeven. Wanneer deze bij zijn target is, vraagt de enemy aan zijn target wat
de volgende target is. Het pad vormt op deze manier iets genaamd een
<a href="https://en.wikipedia.org/wiki/Linked_list" target="_blank">Linked List</a>.
<hr />
Het allermooiste van dit systeem is, is dat ik het ook voor de camera kan gebruiken.
Meestal zie je wel wanneer je in een level komt; dat er een korte "cutscene" is waar
het hele level wordt laten zien.
Het pad van de cutscene kan ik maken middels het zelfde systeem wat ik gebruik voor
de enemies.
![Visual sheet van path finding mechanic](./images/PathFinding.png)