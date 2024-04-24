//To store the user data we got back as a response when we login or sign up
export interface User {
    username: string;
    token: string;
    //For adding the main photo of user to nav bar
    photoUrl: string;
    knownAs: string;
    gender: string;
    roles: string[];
}