import { Component } from '@angular/core';
import { TableModule } from 'primeng/table';

@Component({
  selector: 'app-dashboard',
  imports: [TableModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {
  meetings = [
    { time: '09:00', office: 'Room 101', participants: ['Alice', 'Bob'] },
    { time: '11:00', office: 'Room 102', participants: ['Charlie', 'Dana'] },
    { time: '14:00', office: 'Room 201', participants: ['Eve', 'Frank'] }
  ];

}
