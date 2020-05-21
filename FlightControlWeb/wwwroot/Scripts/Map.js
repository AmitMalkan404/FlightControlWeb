var mymap = L.map('mapid').setView([32.08, 34.78], 10);
L.tileLayer('https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token={accessToken}', {
    attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/">OpenStreetMap</a> contributors, <a href="https://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>, Imagery © <a href="https://www.mapbox.com/">Mapbox</a>',
    maxZoom: 18,
    minZoom: 1,
    id: 'mapbox/streets-v11',
    tileSize: 512,
    zoomOffset: -1,
    accessToken: 'pk.eyJ1IjoicGlrYWNodTIzIiwiYSI6ImNrYTJramNkMDAydWEzZnA5M2l5Zmdnc28ifQ.kw350v1vaUcIs6r6oz8p2g'
}).addTo(mymap);

//Added by Amit
let airplaneIcon = L.icon({
    iconUrl: 'airplane-icon.png',
    iconSize: [30, 30],
    iconAnchor: [0, 0],
});

L.marker([19.02, 57.52], { icon: airplaneIcon }).addTo(mymap);
L.marker([29.72, 35.00], { icon: airplaneIcon }).addTo(mymap);

let latlngs = [
    [[32.01, 34.88],
        [19.02, 57.52],
        [13.77, 100.66]],

    [[29.72, 35.00],
        [32.46, 34.23],
        [34.89, 34.34],
        [34.92, 33.59]]
];
let polyline = L.polyline(latlngs, { color: 'red' }).addTo(mymap);
console.log(typeof polyline);
// zoom the map to the polyline
//map.fitBounds(polyline.getBounds());

// End of Amit's code

function addAirplaneIconToMap(latitude, longitude) {
    L.marker([latitude, longitude], { icon: airplaneIcon }).addTo(mymap);
}

