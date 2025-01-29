import {Component, OnInit} from '@angular/core';
import {Router, RouterOutlet} from '@angular/router';
import {ServerService} from './_services/server.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit{
  title = 'Web';

  constructor(
    private serverService: ServerService,
    private router: Router,
  ) {
  }

  ngOnInit(): void {
    this.serverService.firstStartup().subscribe(firstStartup => {
      if (!firstStartup) {
        return;
      }

      // TODO: Redirect to set up
    })
  }

}
