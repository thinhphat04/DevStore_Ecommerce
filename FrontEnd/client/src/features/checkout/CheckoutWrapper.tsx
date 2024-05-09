import { Elements } from "@stripe/react-stripe-js";
import { loadStripe } from "@stripe/stripe-js";
import { useState, useEffect } from "react";
import agent from "../../app/api/agent";
import LoadingComponent from "../../app/layout/LoadingComponent";
import { useAppDispatch } from "../../app/store/configureStore";
import CheckoutPage from "./CheckoutPage";
import { setCart } from "../cart/CartSlice";

const stripePromise = loadStripe('pk_test_51OlTIyI3XzL9WVaybntAD5rj5RBTEjt506SIR2WIRhXL6TFwJxcDqRbdzUEoD2JVt2bjkworeSXdSttMJti7sxEA00UJ3tYaMK')

export default function CheckoutWrapper() {
    const dispatch = useAppDispatch();
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        agent.Payments.createPaymentIntent()
            .then(cart => dispatch(setCart(cart)))
            .catch(error => console.log(error))
            .finally(() => setLoading(false));
    }, [dispatch]);
    
    if (loading) return <LoadingComponent message="Loading checkout..." />

    return (
        <Elements stripe={stripePromise}>
            <CheckoutPage />
        </Elements>
    )
}