const connection = new signalR.HubConnectionBuilder().withUrl("/chat").withAutomaticReconnect().build();

(async () => {
    try {
        await connection.start();
    } catch (e) {
        console.error(e.toString())
    }
})()