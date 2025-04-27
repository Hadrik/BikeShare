let map = L.map('map').setView([49.83429254048945, 18.161536490538477], 16);

L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 19,
    attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
}).addTo(map);

let stations;

async function fillMap() {
    let stationsRaw = await fetch('api/stations');
    stations = await stationsRaw.json();

    for (let station of stations) {
        let marker = L.marker([station.Latitude, station.Longitude]).addTo(map);
        
        marker.bindPopup(`
            <div>
                <h1>${station.Name}</h1>
                <p>Bikes: <span id="bikes-${station.Id}">loading...</span></p>
                <button id="bikes-${station.Id}-btn" disabled onclick="rent(${station.Id})">Rent a bike</button>
            </div>
        `);
        
        marker.on('click', () => {
            map.panTo(marker.getLatLng());
            refreshPopup(station.Id);
        });
    }
}

let refreshPopup = async (stationId) => {
    let bikesResp = await fetch(`api/stations/${stationId}/bikes`);
    let numberOfBikes = await bikesResp.text();
    document.getElementById(`bikes-${stationId}`).innerText = numberOfBikes;
    let button = document.getElementById(`bikes-${stationId}-btn`);
    if (numberOfBikes > 0) {
        button.disabled = false;
        button.innerText = 'Rent a bike';
    } else {
        button.disabled = true;
        button.innerText = 'No bikes available';
    }
}

async function rent(stationId) {
    let resp = await fetch(`api/rentals/start/${stationId}`, {
        method: 'POST'
    });
    if (!resp.ok) {
        alert(`Failed to start rental. ${await resp.text()}`);
        return;
    }
    await refreshPopup(stationId);
}

fillMap();
