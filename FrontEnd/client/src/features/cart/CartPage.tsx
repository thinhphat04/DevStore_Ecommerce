import {
  Box,
  Button,
  Grid,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import { Add, Delete, Remove } from "@mui/icons-material";
import { LoadingButton } from "@mui/lab";
import CartSummary from "./CartSummary";
import { Link } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../../app/store/configureStore";
import {  addCartItemAsync, removeCartItemAsync } from "./CartSlice";

export default function CartPage() {
  const { cart, status } = useAppSelector(state => state.cart);
  const dispatch = useAppDispatch();


  if (!cart) return <Typography variant="h3">Your Cart is empty</Typography>;

  return (
    <>
      <TableContainer component={Paper}>
        <Table sx={{ minWidth: 650 }}>
          <TableHead>
            <TableRow>
              <TableCell>Product</TableCell>
              <TableCell align="right">Price</TableCell>
              <TableCell align="center">Quantity</TableCell>
              <TableCell align="right">Subtotal</TableCell>
              <TableCell align="right"></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {cart.items.map((i) => (
              <TableRow
                key={i.productId}
                sx={{ "&:last-child td, &:last-child th": { border: 0 } }}
              >
                <TableCell component="th" scope="row">
                  <Box display={"flex"} alignItems={"center"}>
                    <img
                      src={i.pictureUrl}
                      alt={i.name}
                      style={{ height: 50, marginRight: 20 }}
                    />
                    <span>{i.name}</span>
                  </Box>
                </TableCell>
                <TableCell align="right">
                  ${(i.price / 100).toFixed(2)}
                </TableCell>
                <TableCell align="center">
                  <LoadingButton
                    loading={status === ('pendingRemoveItem' + i.productId + 'rem')}
                    onClick={() => dispatch(removeCartItemAsync({
                      productId: i.productId, quantity: 1, name: 'rem'
                    }))}
                    color="error"
                  >
                    <Remove />
                  </LoadingButton>
                  {i.quantity}
                  <LoadingButton
                    loading={status === ('pendingAddItem' + i.productId)}
                    onClick={() => dispatch(addCartItemAsync({productId: i.productId}))}
                    color="secondary"
                  >
                    <Add />
                  </LoadingButton>
                </TableCell>
                <TableCell align="right">
                  ${((i.price / 100) * i.quantity).toFixed(2)}
                </TableCell>
                <TableCell align="right">
                  <LoadingButton
                    loading={status === ('pendingRemoveItem' + i.productId + 'del')}
                    onClick={() => dispatch(removeCartItemAsync({
                      productId: i.productId, quantity: i.quantity, name: 'del'
                    }))}
                    color="error"
                  >
                    <Delete />
                  </LoadingButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
      <Grid container>
        <Grid item xs={6}></Grid>
        <Grid item xs={6}>
          <CartSummary />
          <Button
            component = {Link}
            to = "/checkout"
            variant="contained"
            size="large"
            fullWidth
          >
            Checkout
          </Button>
        </Grid>
      </Grid>
    </>
  );
}
