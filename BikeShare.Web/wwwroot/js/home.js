let map = L.map('map').setView([49.83429254048945, 18.161536490538477], 13);

L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 19,
    attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
}).addTo(map);

async function fillMap() {
    let stations = await fetch('api/stations');
    let stationsData = await stations.json();

    for (let station of stationsData) {
        let marker = L.marker([station.Latitude, station.Longitude]).addTo(map);
        marker.bindPopup(`<b>${station.Name}</b>`);
        marker.on('click', () => {
            window.location.href = `Station/${station.Id}`;
        });
    }
}

fillMap();
