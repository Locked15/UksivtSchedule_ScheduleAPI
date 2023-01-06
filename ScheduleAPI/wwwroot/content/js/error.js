function onLoaded() {
    let baseURL = document.location.origin;

    let refresh = document.createElement('meta');
    refresh.httpEquiv = 'refresh';
    refresh.content = `30; ${baseURL}`;
    document.getElementsByTagName('head')[0].appendChild(refresh);
}
