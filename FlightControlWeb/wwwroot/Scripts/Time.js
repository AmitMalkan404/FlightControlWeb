
/* eslint-disable no-unused-vars */
let allMyFlightsUrl;
/* eslint-enable no-unused-vars */

function getTime() {
    const d = new Date();
    const utcString = d.toUTCString();
    let time = new Date(utcString);
    time = time.toISOString().slice(0, -2);
    // console.log(time);
    //allMyFlightsUrl = `http://localhost:5001/api/Flights?relative_to=${time}Z&sync_all`;
    allMyFlightsUrl = `/api/Flights?relative_to=${time}Z&sync_all`;

}

setInterval(getTime, 500);
