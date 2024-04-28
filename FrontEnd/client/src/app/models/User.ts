import { Cart } from "./Cart";

export interface User {
    email: string,
    token: string,
    cart? : Cart;
    roles?: string[]
}