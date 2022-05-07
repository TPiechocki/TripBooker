import * as React from "react"
import {useEffect, useState} from "react";
import {request} from "../api/request";
import {Button, Card, CardActions, CardContent, CardMedia, Container, Grid, Stack, Typography} from "@mui/material";
import Layout from "../components/Layout";
import {navigate} from "gatsby"


const IndexPage = () => {
  const [data, setData] = useState<any[]>();
  useEffect(() => {
    request('GET', '/Destinations').then(data => {
      setData(data.destinations)
    })
  }, [])
  return (
    <Layout>
      <>
        <Typography variant="h2">
          Destinations
        </Typography>
        <Grid spacing={2} container>
          {data && data.map((destination, i) => <Grid key={destination.airportCode} item xs><Card sx={{minWidth: 200}} className="offerCard">
            <CardMedia
              component="img"
              height="200"
              image={`https://picsum.photos/200/200?random=${i}`}
              alt="destination"
            />
            <CardContent>
              <Typography gutterBottom variant="h5" component="div">
                {destination.name}
              </Typography>
            </CardContent>
            <CardActions>
              <Button size="small" onClick={() => navigate('trips', {state: {destination: destination}})}>
                Check Offers</Button>
            </CardActions>
          </Card></Grid>)}
        </Grid>
      </>
    </Layout>
  )
}

export default IndexPage;
