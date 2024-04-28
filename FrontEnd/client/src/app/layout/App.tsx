import { Container, CssBaseline, ThemeProvider, createTheme } from "@mui/material";
import Header from "./Header";
import { useCallback, useEffect, useState } from "react";
import { Outlet, useLocation } from "react-router-dom";
import { ToastContainer } from "react-toastify";
import 'react-toastify/dist/ReactToastify.css';
import LoadingComponent from "./LoadingComponent";
import { useAppDispatch } from "../store/configureStore";
import { fetchCartAsync } from "../../features/cart/CartSlice";
import { fetchCurrentUser } from "../../features/account/accountSlice";
import HomePage from "../../features/home/HomePage";

function App() {
  const location = useLocation();
  const dispatch = useAppDispatch();
  const [loading, setLoading] = useState(true);

  const initApp = useCallback(async () =>{
    try {
      await dispatch(fetchCurrentUser());
      await dispatch(fetchCartAsync())
    } catch (error) {
      console.log(error)
    }
  }, [dispatch])

  //Get cookie
  useEffect(()=>{
    initApp().then(() => setLoading(false))
  }, [initApp]) 

  const [darkMode, setDarkMode] = useState(false);
  const paletteType = darkMode ? 'dark' : 'light';
  const theme = createTheme({
    palette:{
      mode: paletteType,
      background:{
        default: paletteType === 'light'? '#eaeaea' : '#121212'
      }
    }
  });

  function handleThemeChange (){
    setDarkMode(!darkMode)
  }

  return (
    <ThemeProvider theme={theme}>
      <ToastContainer position="bottom-right"
        autoClose={5000}
        hideProgressBar={false}
        newestOnTop={false}
        closeOnClick
        rtl={false}
        pauseOnFocusLoss
        draggable
        pauseOnHover 
        theme="colored"/>
      <CssBaseline />
      <Header darkMode={darkMode} handleThemeChange={handleThemeChange} />
      {loading ? <LoadingComponent message="Initialising app..." />
          : location.pathname === '/' ? <HomePage />
          : <Container sx={{mt: 4}}>
              <Outlet />
            </Container>
      }
    </ThemeProvider>
  );
}

export default App;
