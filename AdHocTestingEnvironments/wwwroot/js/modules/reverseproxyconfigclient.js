export { getRoutes, createRoute, deleteRoute };

const api = window.apiPrefix;

async function getRoutes() {
    const url = `${api}/Routing`;
    var res = await fetch(url);
    const json = await res.json();    
    return json;
}

async function createRoute(data) {
    const url = `${api}/Routing/`;
    const res = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    });
    const json = await res.json();
    return json;
}

async function deleteRoute(id) {
    const url = `${api}/Routing/${id}`;
    const res = await fetch(url, {
        method: 'DELETE',
    });
}