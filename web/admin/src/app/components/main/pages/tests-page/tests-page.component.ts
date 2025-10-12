import { Component, OnInit } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { TestService } from '../../../../services/API/test.service';
import { UpdatableMap } from '../../../../utils/utils';
import { ITest } from '../../../../models/entity/test';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { CreateTestModalComponent } from './create-test-modal/create-test-modal.component';
import { HubClientService } from '../../../../services/API/hub-client.service';
import { Subscription } from 'rxjs';
import { EditTestModalComponent } from './edit-test-modal/edit-test-modal.component';

@Component({
  selector: 'app-tests-page',
  standalone: true,
  imports: [TranslateModule],
  templateUrl: './tests-page.component.html',
  styleUrl: './tests-page.component.css'
})
export class TestsPageComponent implements OnInit {
  protected tests: UpdatableMap<ITest> = new UpdatableMap<ITest>(t => String(t.id));

  protected createTestModalRef?: BsModalRef;
  protected editTestModalRef?: BsModalRef;

  protected testUpdateSubscription?: Subscription;

  constructor(private testService: TestService,
              private modalService: BsModalService,
              private hubClient: HubClientService) {}

  async ngOnInit() {
    this.tests.setElems(await this.testService.getAll());

    this.testUpdateSubscription = this.hubClient.testDataUpdates.subscribe(test => {
      this.tests.onUpdate(test);
    });
  }

  protected openCreateTestModal() {
    this.createTestModalRef = this.modalService.show(CreateTestModalComponent);
  }

  protected openEditTestModal(test: ITest) {
      const initialState: ModalOptions = {
        initialState: {
          test: test
        }
      };
      this.editTestModalRef = this.modalService.show(EditTestModalComponent, initialState);
  }
}
