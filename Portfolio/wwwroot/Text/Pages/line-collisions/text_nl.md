## Introductie {#iductie}
Line collisions zijn een hele belangrijke vorm/basis van collisions.
<hr/>
Een vast regel bij line collisions is "Hoek van inval, is hoek van uitval".
Deze is het makkelijkst toe te passen bij perfecte horizontale of verticale lijnen.
<hr/>
<p>
Een andere manier om dit te berekenen, is middels
<a
    href="https://www.physicsclassroom.com/class/vectors/Lesson-1/Vector-Resolution"
    target="_blank">
        Vector Resolution
</a>
en lijkt heel erg op het berekenen van de
<a
    href="https://www.mrchadd.nl/academy/vakken/wiskunde/de-richtingscoefficient"
    target="_blank">
        De Richtingscoëfficient
</a>
van een lineare formule.
</p>
  
![Hoek van inval is hoek van uitval](./images/circle-line-2.png "open")
![Simpele uitwerking van Vector Resolution](./images/vector-resolution.png "open")

## Uitwerking {#uit}
<p>
Een voorbeelduitwerking van hoe ik dit toegepast heb,
lijkt enorm op vector resolution. Alleen in mijn geval houdt het rekening met de hoek waronder
de lijn staat.
</p>

<hr/>

Laten we beginnen met een lijn die onder een hoek van 45° staat, en
een bal die deze tegemoet komt met een richting van
![Vector v = [0.89; -0.44]](./images/math/line-collisions/vector_v.svg)
De normal van de lijn is als volgt:
![Vector n = [-0.71; 0.71]](./images/math/line-collisions/vector_n.svg)
ook hebben we een vector nodig
die de lijn navolgt:
![Vector a = [0.71; 0.71]](./images/math/line-collisions/vector_a.svg)
Om met deze twee vectoren een "bounding-box"
te vormen, vermenigvuldigen we deze met de dotproduct. hieruit volgt weer een scalair,
dus vermenigvuldigen we hiervan de uitkomst weer met de corresponderende vector. Als voorbeeld de normal: <br/>
nieuwe normal (nn):  
![Vector nn = [0.67, -0.67]](./images/math/line-collisions/vector_nn_equation.svg)  
nieuwe along (na):  
![Vector na = [0.23, 0.23]](./images/math/line-collisions/vector_na_equation.svg)  
Nu hebben we een resolutie-vector van `v`, die rekening houdt met de normal van de lijn.
De nieuwe velocity `v` na impact wordt als volgt berekent: <br/>
![Vector nv = [-0.44, 0.89]](./images/math/line-collisions/vector_nv_equation.svg)  
  
![Resolution process](./images/resolution-process.png "open")

## Achterliggend {#systeem}
Ik heb, als achterliggend systeem, een singleton gebouwd waar ik aan het begin van de scene
alle lijnen bij kan registreren. Later tijdens het spelen, kan de bal checken bij deze singleton of
hij in de buurt komt van een lijn. Hier kan hij de normal van de lijn opvragen en de richting van de lijn.
Deze info is, zoals net aangegeven, nodig voor het berekenen van de nieuwe velocity na impact.

<hr/>

Ook heb ik, zoals je in het vorige plaatje kon zien, een 
\`<a
    href="https://github.com/MitchelMA/Line-collision/blob/revision/lineparts/Assets/Scripts/Utils/PqrForm.cs"
    target="_blank">
PqrForm
</a>\`
gemaakt. dit is een class
die ik gebruik om wiskundige berekeningen te maken met de componenten p, q en r in de
opbouw: `px + qy = r`.
Ik kan met deze class de slope van een formule berekenen, de angle, ik kan de x ophalen met
een gegeven y en andersom.. en nog veel meer.  
![De add method in de singleton](./images/rider64_SUVtMKrW1o.png "open")