## Aandeel {#aandeel}
<p style="text-align: justify;">
Mijn aandeel in dit project bestond uit het maken van de boundaries, de camera en de enemy ai.
We hadden eigen boundaries gebruikt omdat die van Unity niet helemaal lekker werkte, we hadden
en de camera zo geschreven dat hij mooi onder een hoek stond en focust op een bepaald punt en
de enemy-ai logischerwijs zodat je tegenstanders had om tegen te spelen.
</p>

## Showreel {#reel}
Dit is een showreel van tijdens development:  
<video controls
       poster="./images/Unity_KFo5HjMIzQ.png"
       thumbnail="./images/Unity_KFo5HjMIzQ.png"
       width="100%"
       style="border-radius: var(--border-radius-small)"> <source src="./videos/DAReel.mp4" type="video/mp4"/>
</video>

## Camera {#camera}
Bij het instellen van de camera, heb ik het zo geschreven dat de character-sprites automatisch naar
de camera toe draaien.  

Bij het instellen van de camera hoef je namelijk alleen maar een hoek door tegeven, en dan staat
de camera automatisch gedraaid, kijkend naar het goede punt.
Daarom is het ook makkelijk als de sprites ook gedraaid zijn naar de camera.  

Omdat ik hiermee de hoek bereken tussen sprite en camera, kan ik ook tegelijkertijd corrigeren
voor de hoogte, waardoor de sprite nogsteeds op de grond staat.  
  
![Visual sheet van het naar de camera toe draaien](./images/FaceCam.png)

## Boundaries {#bounds}
Dit spel maakt ook gebruik van "bounds".  
We hadden er namelijk voor gekozen om geen ingebouwde colliders te gebruiken van Unity.
Deze werkte namelijk niet goed voor wat wij deden waardoor de bal raar begon te doen.  

Hier zie je een visual sheet van hoe het instellen van deze bounds eruit ziet.
Je hebt de outline die blauw is en de middelijn die rood is. Ook kan je hem verplaatsen door 
"Centre" te veranderen, wat een 2 dimensionale vector is.  

Ook zijn er dingen toegevoegd om te zorgen dat alle dingen die bewegen automatisch rekening
houden met deze ingestelde bounds.
En ook kan je de bounds ophalen van aan welke kant je staat (rechts of links).
Dit omdat de middelijn ook ingesteld kan worden.  
  
![Visual sheet van het instellen van de boundaries](./images/Bounds.png)