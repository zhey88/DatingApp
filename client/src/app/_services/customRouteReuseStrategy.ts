import { ActivatedRouteSnapshot, DetachedRouteHandle, RouteReuseStrategy } from "@angular/router";

//When user A is connected to a message hub with user B, 
//And user C sent a message to user A, A clicked on the message pop out
//Directed to message hub with user C but the message tab does not work 

//To solve this problem, we need to use of the RouteReuseStrategy
export class CustomRouteReuseStrategy implements RouteReuseStrategy {
    shouldDetach(route: ActivatedRouteSnapshot): boolean {
        return false;
    }
    store(route: ActivatedRouteSnapshot, handle: DetachedRouteHandle | null): void {
    }
    shouldAttach(route: ActivatedRouteSnapshot): boolean {
        return false;
    }
    retrieve(route: ActivatedRouteSnapshot): DetachedRouteHandle | null {
        return null;
    }
    shouldReuseRoute(future: ActivatedRouteSnapshot, curr: ActivatedRouteSnapshot): boolean {
        return false;
    }

}