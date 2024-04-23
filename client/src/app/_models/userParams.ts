import { User } from "./user";

export class UserParams {
    gender: string;
    minAge = 18;
    maxAge = 99;
    pageNumber = 1;
    pageSize = 5;
    orderBy = 'lastActive';

    //If the logged in user is female, set this gender to be male
    //this means that if the user's gender is female, then we're going to request male users
    constructor(user: User) {
        this.gender  = user.gender === 'female' ? 'male' : 'female'
    }
}