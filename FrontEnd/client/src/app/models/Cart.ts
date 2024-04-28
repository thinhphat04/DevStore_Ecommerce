export interface Cart {
    id: number;
    buyerId: string;
    items: CartItem[];
    paymentIntentId?: string;
    clientSecret?: string;
  }
  
  export interface CartItem {
    productId: number;
    name: string;
    price: number;
    pictureUrl: string;
    type: string;
    brand: string;
    quantity: number;
  }