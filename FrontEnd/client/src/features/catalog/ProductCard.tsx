import { Avatar, Button, Card, CardActions, CardContent, CardHeader, CardMedia, Typography } from "@mui/material";
import { Product } from "../../app/models/Product";
import { Link } from "react-router-dom";
import { LoadingButton } from "@mui/lab";
import { currencyFormat } from "../../app/util/util";
import { useAppDispatch, useAppSelector } from "../../app/store/configureStore";
import { addCartItemAsync } from "../cart/CartSlice";


interface Props{
    product : Product;
}

export default function ProductCard({product} : Props){
  const {status} = useAppSelector(state => state.cart);
  const dispatch = useAppDispatch();

    return(
      <Card >
        <CardHeader 
          avatar={
            <Avatar sx={{bgcolor:'secondary.main'}}>
              {product.name.charAt(0).toUpperCase()}
            </Avatar>           
          }
          title={product.name}
          titleTypographyProps={{
            sx:{fontWeight: 'bold', color: 'primary.main'}
          }}
        />
      <CardMedia
        sx={{ height: 140, backgroundSize: 'contain'}}
        image={product.pictureUrl}
        title={product.name}
      />
      <CardContent>
        <Typography gutterBottom variant="h5" color='secondary'>
          {currencyFormat(product.price)}
        </Typography>
        <Typography variant="body2" color="text.secondary">
          {product.brand} / {product.type}
        </Typography>
      </CardContent>
      <CardActions>
        <LoadingButton 
        loading = {status === 'pendingAddItem' + product.id}
          onClick={() => dispatch(addCartItemAsync({productId :product.id}))} 
          size="small">Add to Cart</LoadingButton>
        <Button size="small" component={Link} to={`/catalog/${product.id}`} >View</Button>
      </CardActions>
    </Card>
    )
}