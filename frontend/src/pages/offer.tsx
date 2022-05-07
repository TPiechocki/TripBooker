import React from 'react';
import {PageProps} from "gatsby";

interface State {
  airportCode: string,
  departureDate: Date,
  numberOfDays: string,
  numberOfAdults: string,
  numberOfChildrenUpTo18: string,
  numberOfChildrenUpTo10: string,
  numberOfChildrenUpTo3: string,
  trip: {
    hotelCode: string
    hotelName: string,
    minimalPrice: number
  }
}

const offer = ({location}: PageProps<{}, any, State | any>) => {
  const {state} = location;
  console.log(state);
  return (
    <>
      Offer
    </>
  );
};

export default offer;