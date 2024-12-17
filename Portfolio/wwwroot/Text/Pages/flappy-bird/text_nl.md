## Introductie {#iductie}
<p style="text-align: justify;">
Tijdens mijn stage bij het Simulatie Centrum Maritiem (SimCenMar) heb ik ook gewerkt aan een leerdoelen project.
Eén van mijn leerdoelen was dat ik graag C++ wilde leren in combinatie met de Unreal Engine.
</p>

## Object Pooling {#obj-pool}
Het spel Flappy Bird is een spel dat oneindig lang doorgaat. Dit betekend dat als ik niet van een systeem gebruik maak
wat de obstakels opruimt, er daar ook oneindig veel van gaan zijn.  
![Visual sheet van object pooling](./images/flappy-bird/ObstaclePooling.png "open")  
Dit systeem bestaat uit het object wat de obstakels opslaat, een spawner en een opruimer, aangegeven met "Trigger".
Dit systeem maakt het voor mij mogelijk om maximaal maar 3 à 4 obstakels tegelijkertijd te hebben omdat ze steeds opnieuw
worden gebruikt.