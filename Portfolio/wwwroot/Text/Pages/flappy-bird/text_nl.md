## Introductie {#iductie}
<p style="text-align: justify;">
Tijdens mijn stage bij het Simulatie Centrum Maritiem (SimCenMar) heb ik ook gewerkt aan een leerdoelen project.
Eén van mijn leerdoelen was dat ik graag C++ wilde leren in combinatie met de Unreal Engine.
</p>

## Gameplay {#game}
Dit is een korte video van hoe de gameplay er uit ziet.  
<video controls
       poster="./images/flappy-bird/thumbnail.png"
       thumbnail="./images/flappy-bird/thumbnail.png"> <source src="./videos/flappy-bird/2024-06-08%2018-06-50(cropped).mp4" type="video/mp4"/>
</video>

## Object Pooling {#obj-pool}
Het spel Flappy Bird is een spel dat oneindig lang doorgaat. Dit betekend dat als ik niet van een systeem gebruik maak
wat de obstakels opruimt, er daar ook oneindig veel van gaan zijn.  
Dit systeem bestaat uit het object wat de obstakels opslaat, een spawner en een opruimer, aangegeven met "Trigger".
Dit systeem maakt het voor mij mogelijk om maximaal maar 3 à 4 obstakels tegelijkertijd te hebben omdat ze steeds opnieuw
worden gebruikt.  
![Visual sheet van object pooling](./images/flappy-bird/ObstaclePooling.png "open")  
