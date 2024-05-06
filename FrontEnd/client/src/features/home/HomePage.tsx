import { Box, Typography } from "@mui/material";
import Slider from "react-slick";

export default function HomePage() {
  var settings = {
    dots: true,
    infinite: true,
    speed: 500,
    slidesToShow: 1,
    slidesToScroll: 1,
  };
  return (
    <>
      <Slider {...settings}>
        <div>
          <img
            src="/images/homepage1.jpg"
            alt="hero"
            style={{ display: "block", width: "100%", maxHeight: 600 }}
          />
        </div>
        <div>
          <img
            src="/images/homepage2.jpg"
            alt="hero"
            style={{ display: "block", width: "100%", maxHeight: 600 }}
          />
        </div>
        <div>
          <img
            src="/images/homepage3.jpg"
            alt="hero"
            style={{ display: "block", width: "100%", maxHeight: 600 }}
          />
        </div>
      </Slider>
      <Box display='flex' justifyContent='center' sx={{ p: 4 }}>
        <Typography variant='h3'>
          Welcome to the DEV-Store E-commerce Website
        </Typography>
      </Box>
      <Box display='flex' justifyContent='center' sx={{ p: 4 }}>
        <Typography variant='h4'>
          SourceCode: <a href="https://github.com/thinhphat04/DevStore_Ecommerce">https://github.com/thinhphat04/DevStore_Ecommerce</a>
        </Typography>
      </Box>

    </>
  );
}
