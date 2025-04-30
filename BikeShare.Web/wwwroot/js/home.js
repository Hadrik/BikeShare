let map = L.map('map').setView([49.83429254048945, 18.161536490538477], 16);

L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 19,
    attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
}).addTo(map);

let stations;
let activeRental = false;

async function fillMap() {
    let stations = await(await fetch('api/stations')).json();
    
    let rental = await(await fetch('api/rentals/active')).json();
    if (rental) activeRental = true;

    for (let station of stations) {
        let marker = L.marker([station.latitude, station.longitude]).addTo(map);
        
        marker.bindPopup(`
            <div>
                <h1>${station.name}</h1>
                <p>Bikes: <span id="bikes-${station.id}">loading...</span></p>
                <button id="bikes-${station.id}-btn" disabled onclick="rent(${station.id})">Rent a bike</button>
            </div>
        `);
        
        marker.on('click', () => {
            map.panTo(marker.getLatLng());
            refreshPopup(station.id);
        });
    }
}

let refreshPopup = async (stationId) => {
    let bikesResp = await fetch(`api/stations/${stationId}/bikes`);
    let numberOfBikes = await bikesResp.text();
    document.getElementById(`bikes-${stationId}`).innerText = numberOfBikes;
    let button = document.getElementById(`bikes-${stationId}-btn`);
    
    if (activeRental) {
        button.disabled = false;
        button.innerText = 'Return bike';
        button.onclick = () => returnBike(stationId);
    } else {
        if (numberOfBikes > 0) {
            button.disabled = false;
            button.innerText = 'Rent a bike';
        } else {
            button.disabled = true;
            button.innerText = 'No bikes available';
        }
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
    // TODO: Success message
    activeRental = true;
    await refreshPopup(stationId);
}

async function returnBike(stationId) {
    let resp = await fetch(`api/rentals/end/${stationId}`, {
        method: 'POST'
    });
    if (!resp.ok) {
        alert(`Failed to return bike. ${await resp.text()}`);
        return;
    }
    activeRental = false;
    refreshPopup(stationId);
}

fillMap();
