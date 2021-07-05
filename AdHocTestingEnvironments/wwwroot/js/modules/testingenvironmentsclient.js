export { listEnvironments, listEnvironmentInstances, startApplication, stopApplication };
const api = window.apiPrefix;

async function listEnvironments() {
    const url = `${api}/AdHocTestingEnvironments/ListEnvironments`;
    var res = await fetch(url);
    const json = await res.json();
    return json;
}


async function listEnvironmentInstances() {
    const url = `${api}/AdHocTestingEnvironments`;
    var res = await fetch(url);
    const json = await res.json();
    return json;
}

async function startApplication(name, numHours) {
    const url = `${api}/AdHocTestingEnvironments`;
    const res = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            applicationName: name,
            numHoursToRun: numHours,
        }),
    });
    const json = await res.text();
    return json;
}

async function stopApplication(instancename) {
   const url = `${api}/AdHocTestingEnvironments/${instancename}`;
   const res = await fetch(url, {
        method: 'DELETE',
   });
}
