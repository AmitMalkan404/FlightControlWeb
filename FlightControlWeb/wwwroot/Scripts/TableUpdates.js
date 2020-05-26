//let allMyFlightsUrl = "https://localhost:5001/api/Flights?relative_to=2020-05-25T10:30:00Z&sync_all";

let allMyFlightsUrl;

function getTime() {
    let d = new Date();
    let utcString = d.toUTCString();
    let time = new Date(utcString);
    time = time.toISOString().slice(0, -2);
    console.log(time);
    allMyFlightsUrl = "https://localhost:5001/api/Flights?relative_to=" + time + "Z&sync_all";
}

setInterval(getTime, 500);
