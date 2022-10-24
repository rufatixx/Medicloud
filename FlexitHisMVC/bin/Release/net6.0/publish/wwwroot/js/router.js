var appURL = "https://api.flexit.az"
var devAppURL = "https://localhost:44300";


//const route = (event) => {
//    event = event || window.event;
//    event.preventDefault();
//    window.history.pushState({}, "", event.target.href);
//    handleLocation();
//};
//const routes = {
//    404:"/menu",
   
//    "/admin/company_settings": "/Admin/Companies",
//    "/": "/Admin/Companies",
//    "/np": "/new_patient",
//    "/kMenu": "/kassa_menu",
//    "/paidChecks": "/kassa",
//    "/admin/deps": "/Admin/Departments",
   
//};
//const handleLocation = async () => {
//    const path = window.location.pathname;

//    const rt = routes[path] || routes[404];
//        //const html = await fetch(r).then((data) => data.text());
//        $("#main-content").load(rt);
//};
//window.onpopstate = handleLocation;
//window.route = route;
//handleLocation();