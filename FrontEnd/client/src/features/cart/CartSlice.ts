import { createAsyncThunk, createSlice, isAnyOf } from "@reduxjs/toolkit";
import { Cart } from "../../app/models/Cart";
import agent from "../../app/api/agent";
import { getCookie } from "../../app/util/util";

interface CartState{
    cart: Cart | null,
    status: string
}

const initialState: CartState ={
    cart: null,
    status: 'idle'
}

export const fetchCartAsync = createAsyncThunk<Cart>(
    'cart/fetchCartAsync',
    async(_, thunkAPI) => {
        try {
            return await agent.Cart.get();
        } catch (error: any) {
            return thunkAPI.rejectWithValue({error: error.data})
        }
    }, {
        condition: () =>{
            if(!getCookie('buyerId')) return false;
        }
    }
)

export const addCartItemAsync = createAsyncThunk<Cart, {productId: number, quantity?: number}>(
    'cart/addCartItemAsync',
    async({productId, quantity =1}, thunkAPI) => {
        try{    
            return await agent.Cart.addItem(productId, quantity);
        } catch(error: any){
            return thunkAPI.rejectWithValue({error: error.data})
        }
    }
) 

export const removeCartItemAsync = createAsyncThunk<void, {productId: number, quantity: number, name?: string}>(
    'cart/removeItemAsync',
    async({productId, quantity}, thunkAPI) =>{
        try{    
            return await agent.Cart.removeItem(productId, quantity);
        } catch(error: any){
            return thunkAPI.rejectWithValue({error: error.data});
        }
    }
)


export const cartSlice = createSlice({
    name: 'cart',
    initialState,
    reducers: {
        setCart: (state, action) =>{
            state.cart = action.payload
        },
        clearCart: (state) => {
            state.cart = null;
        }
    },
    extraReducers: (builder => {
        builder.addCase(addCartItemAsync.pending, (state, action) =>{
            console.log(action);
            state.status = 'pendingAddItem' + action.meta.arg.productId;
        });
        
        builder.addCase(removeCartItemAsync.pending, (state, action)=>{
            state.status = 'pendingRemoveItem' + action.meta.arg.productId +action.meta.arg.name;
        });
        builder.addCase(removeCartItemAsync.fulfilled, (state, action)=>{
            const {productId, quantity} = action.meta.arg;
            const itemIndex = state.cart?.items.findIndex(i => i.productId === productId);
            if(itemIndex === -1 || itemIndex === undefined) return;
            state.cart!.items[itemIndex].quantity -= quantity!;
            if(state.cart?.items[itemIndex].quantity ===0) 
                state.cart.items.splice(itemIndex, 1);
            state.status = 'idle';
        })
        builder.addCase(removeCartItemAsync.rejected, (state, action)=>{
            state.status = 'idle';
            console.log(action.payload);
        });
        builder.addMatcher(isAnyOf(addCartItemAsync.fulfilled, fetchCartAsync.fulfilled), (state, action) =>{
            state.cart = action.payload;
            state.status = 'idle'
        });
        builder.addMatcher(isAnyOf(addCartItemAsync.rejected, fetchCartAsync.rejected), (state, action) =>{
            state.status = 'idle';
            console.log(action.payload);
        });
    })
})

export const {setCart, clearCart} = cartSlice.actions;