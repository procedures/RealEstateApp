// Хамгийн энгийн Service Worker — цорын ганц зорилго нь браузерт энэ сайтыг
// "Add to Home Screen" (утасны дэлгэц дээр shortcut) хийх боломжтойг мэдэгдэх.
// Blazor Server апп тасралтгүй сервертэй холбоо (SignalR) шаарддаг тул
// офлайн горим/кэш хийхгүй — бүх хүсэлтийг энгийнээр сүлжээгээр дамжуулна.

self.addEventListener('install', () => {
    self.skipWaiting();
});

self.addEventListener('activate', (event) => {
    event.waitUntil(self.clients.claim());
});

self.addEventListener('fetch', (event) => {
    event.respondWith(fetch(event.request));
});
