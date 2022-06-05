import * as React from "react"
import {useEffect, useState} from "react";
import {request} from "../api/request";
import {Button, Card, CardActions, CardContent, CardMedia, Container, Grid, Stack, Typography} from "@mui/material";
import Layout from "../components/Layout";
import {navigate} from "gatsby"
import {HubConnection, HubConnectionBuilder, LogLevel} from "@microsoft/signalr";


const IndexPage = () => {
  const [data, setData] = useState<any[]>();
  useEffect(() => {
    request('GET', '/Destinations').then(data => {
      setData(data.destinations)
    })
  }, [])

  const [connection, setConnection] = useState<HubConnection | null>(null);

  const [counts, setCounts] = useState<{ [key: string]: number }>({})

  useEffect(() => {
    const purchasedNotificationsHubConnection = new HubConnectionBuilder()
      .withUrl(process.env.WEB_API_URL + 'destinationsHub')
      .configureLogging(LogLevel.Warning)
      .withAutomaticReconnect()
      .build();

    setConnection(purchasedNotificationsHubConnection);
  }, []);

  useEffect(() => {
    if (connection) {
      connection.off("DestinationCountUpdate");
      connection.on("DestinationCountUpdate", (response: { destinationAirportCode: string, newCount: number }) => {
        setCounts({...counts, [response.destinationAirportCode]: response.newCount})
      })
    }
  }, [connection, counts])

  useEffect(() => {
    if (connection) {
      connection.start()
        .then(result => {
          console.log('Connected!');
          connection.on("DestinationCountsResponse", (response: { counts: { [key: string]: number } }) => {
            setCounts(response.counts)
          })
          connection.invoke("GetAll");
        })
        .catch(e => console.log('Connection failed: ', e));
      return () => {
        connection.stop().then(() => {
          console.log("Connection stopped");
        });
      }
    }
  }, [connection])

  return (
    <Layout>
      <>
        <Typography variant="h2">
          Destinations
        </Typography>
        <Grid spacing={2} container>
          {data && data.map((destination, i) => <Grid key={destination.airportCode} item xs>
            <Card
              sx={{minWidth: 200}}
              className="offerCard"
            >
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
                {counts[destination.airportCode] ? <Typography>
                  {`${counts[destination.airportCode]} ${counts[destination.airportCode] > 1 ? 'users' : 'user'} just booked offer for this destination!`}
                </Typography> : ''}
              </CardContent>
              <CardActions>
                <Button size="small" onClick={() => navigate('trips', {state: {destination: destination}})}>
                  Check Offers</Button>
              </CardActions>
            </Card>
          </Grid>)}
        </Grid>
      </>
    </Layout>
  )
}

export default IndexPage;
