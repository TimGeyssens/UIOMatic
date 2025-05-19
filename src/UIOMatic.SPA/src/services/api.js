async function quickFetch(endpoint) {
  const response = await fetch(endpoint);
  const json = await response.json();
  return json;
}
async function quickPut(endpoint, data) {
    const response = await fetch(endpoint, { method: "PUT", headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(data) });
    const json = await response.json();
    return json;
}

export default function makeService(contentType) {
  return {
    async find(id) {
       return quickFetch(`${contentType.endpoint}/GetById/typeAlias=${contentType.name}&id=${id}`);
    },
    async list() {
        return quickFetch(`${contentType.endpoint}/GetAll?typeAlias=${contentType.name}&sortColumn=Id&sortOrder=desc`);
      },
      async update(object) {
          let data = {}
          data.typeAlias = contentType.name;
          data.value = object;
        return quickPut(`${contentType.endpoint}/Update`,data)
    }

  };
}
