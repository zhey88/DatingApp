import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';
//To make the http request
import { HttpClientModule } from '@angular/common/http';
//import { AppRoutingModule } from './app-routing.module'
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

//To add all the components
@NgModule({
  declarations: [
    AppComponent,
  ],

  //To add new modules
  imports: [
    BrowserModule,
    //AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
