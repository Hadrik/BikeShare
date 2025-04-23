let map = L.map('map').setView([49.83429254048945, 18.161536490538477], 16);

L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 19,
    attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
}).addTo(map);

async function fillMap() {
    let stations = await fetch('api/stations');
    let stationsData = await stations.json();

    for (let station of stationsData) {
        let marker = L.marker([station.Latitude, station.Longitude]).addTo(map);
        
        marker.bindPopup(`
            <div>
                <h1>${station.Name}</h1>
                <p>Bikes: <span id="bikes-${station.Id}">loading...</span></p>
                <button id="bikes-${station.Id}-btn" disabled onclick="rent(${station.Id})">Rent a bike</button>
            </div>
        `);
        
        marker.on('popupopen', async () => {
            let bikesResp = await fetch(`api/stations/${station.Id}/bikes`);
            let numberOfBikes = await bikesResp.text();
            document.getElementById(`bikes-${station.Id}`).innerText = numberOfBikes;
            let button = document.getElementById(`bikes-${station.Id}-btn`);
            if (numberOfBikes > 0) {
                button.disabled = false;
                button.innerText = 'Rent a bike';
            } else {
                button.disabled = true;
                button.innerText = 'No bikes available';
            }
        });
        
        marker.on('click', () => {
            map.panTo(marker.getLatLng());
        });
    }
}

async function rent(stationId) {
    let resp = await fetch(`api/rentals/start/${stationId}`, {
        method: 'POST'
    });
    if (resp.ok) {
        let rental = await resp.json();
        alert(`Rental started! Rental ID: ${rental.Id}`);
    } else {
        alert(`Failed to start rental. ${await resp.text()}`);
    }
}

fillMap();
