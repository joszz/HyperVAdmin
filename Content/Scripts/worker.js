/**
* Just a stub worker file, to make this project installable as PWA.
*/

self.addEventListener("install", function (_event) {
    self.skipWaiting();
});

self.addEventListener("activate", function(_event) {
    return self.clients.claim();
});

self.addEventListener("fetch", function (_event) {
    return;
});